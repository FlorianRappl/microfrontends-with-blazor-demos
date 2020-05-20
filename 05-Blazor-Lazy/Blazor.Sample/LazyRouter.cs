using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace Blazor.Sample
{
	public class LazyRouter : IComponent, IHandleAfterRender, IDisposable, IRouterEnvelope
	{
		readonly Router _router = new Router();
		readonly static MethodInfo _routerNavigationManagerGetter, _routerNavigationManagerSetter;
		readonly static MethodInfo _routerNavigationInterceptionGetter, _routerNavigationInterceptionSetter;
		readonly static MethodInfo _routerLoggerFactoryGetter, _routerLoggerFactorySetter;

		static LazyRouter()
		{
			Type type = typeof(Router);
			PropertyInfo pi = type.GetProperty("NavigationManager", BindingFlags.NonPublic | BindingFlags.Instance);
			_routerNavigationManagerGetter = pi.GetGetMethod(true);
			_routerNavigationManagerSetter = pi.GetSetMethod(true);
			pi = type.GetProperty("NavigationInterception", BindingFlags.NonPublic | BindingFlags.Instance);
			_routerNavigationInterceptionGetter = pi.GetGetMethod(true);
			_routerNavigationInterceptionSetter = pi.GetSetMethod(true);
			pi = type.GetProperty("LoggerFactory", BindingFlags.NonPublic | BindingFlags.Instance);
			_routerLoggerFactoryGetter = pi.GetGetMethod(true);
			_routerLoggerFactorySetter = pi.GetSetMethod(true);
		}

#pragma warning disable BL0005
		[Parameter]
		public Assembly AppAssembly { get { return _router.AppAssembly; } set { _router.AppAssembly = value; } }

		[Parameter]
		public IEnumerable<Assembly> AdditionalAssemblies { get { return _router.AdditionalAssemblies; } set { _router.AdditionalAssemblies = value; } }

		[Parameter]
		public RenderFragment NotFound { get { return _router.NotFound; } set { _router.NotFound = value; } }

		[Parameter]
		public RenderFragment<RouteData> Found { get { return _router.Found; } set { _router.Found = value; } }
#pragma warning restore BL0005

		[Inject]
		private NavigationManager NavigationManager
		{
			get => (NavigationManager)_routerNavigationManagerGetter.Invoke(_router, new object[0]);
			set => _routerNavigationManagerSetter.Invoke(_router, new object[] { value });
		}

		[Inject]
		private INavigationInterception NavigationInterception
		{
			get => (INavigationInterception)_routerNavigationInterceptionGetter.Invoke(_router, new object[0]);
			set => _routerNavigationInterceptionSetter.Invoke(_router, new object[] { value });
		}

		[Inject]
		private ILoggerFactory LoggerFactory
		{
			get => (ILoggerFactory)_routerLoggerFactoryGetter.Invoke(_router, new object[0]);
			set => _routerLoggerFactorySetter.Invoke(_router, new object[] { value });
		}

		[Inject] IAssemblyLazyLoadResolver AssemblyLazyLoadResolver { get; set; }

		public void Attach(RenderHandle renderHandle)
			=> _router.Attach(renderHandle);

		public void Dispose()
			=> _router.Dispose();

		int _phase = 0;
		IReadOnlyDictionary<string, object> _parms = null;

		public async Task SetParametersAsync(ParameterView parameters)
		{
			if (_phase == 0) //called from Blazor
			{
				JSInteropMethods.Router = this;
				parameters.SetParameterProperties(this);
				_parms = parameters.ToDictionary();
				_phase = 1;
				string url = new Uri(NavigationManager.Uri, UriKind.RelativeOrAbsolute).AbsoluteUri;
				await AssemblyLazyLoadResolver.ResolveAsync(url, true);
				_phase = 2;
				await _router.SetParametersAsync(ParameterView.FromDictionary(_parms.ToDictionary(m => m.Key, m => m.Value)));
			}
			else if (_phase == 1) //called from AssemblyLazyLoadResolver
				_parms = parameters.ToDictionary();
			else //through the parameters directly to the real router
				await _router.SetParametersAsync(parameters);
		}

		public Task OnAfterRenderAsync()
			=> ((IHandleAfterRender)_router).OnAfterRenderAsync();

		[Inject] HttpClient HttpClient { get; set; }
	}

	public interface IRouterEnvelope : IComponent
	{
		IEnumerable<Assembly> AdditionalAssemblies { get; set; }
	}
}
