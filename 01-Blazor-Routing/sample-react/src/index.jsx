import React from 'react';
import { render } from 'react-dom';
import { BrowserRouter, Switch, Route } from 'react-router-dom';

const Foo = () => (
    <div>On the foo page from React.</div>
);

const Bar = () => (
    <div>On the bar page from React.</div>
);

const App = () => (
    <BrowserRouter>
        <Switch>
            <Route path="/foo" exact component={Foo} />
            <Route path="/bar" exact component={Bar} />
        </Switch>
    </BrowserRouter>
);

render(<App />, document.querySelector('#react-app'));
