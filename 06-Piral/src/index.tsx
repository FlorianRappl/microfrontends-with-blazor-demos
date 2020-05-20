import 'piral/polyfills';
import { renderInstance } from 'piral';
import { createBlazorApi } from 'piral-blazor';
import { layout, errors } from './layout';

// change to your feed URL here (either using feed.piral.cloud or your own service)
const feedUrl = 'https://feed.piral.cloud/api/v1/pilet/empty';

renderInstance({
  layout,
  errors,
  extendApi: [createBlazorApi()],
  requestPilets() {
    return fetch(feedUrl)
      .then(res => res.json())
      .then(res => res.items);
  },
});
