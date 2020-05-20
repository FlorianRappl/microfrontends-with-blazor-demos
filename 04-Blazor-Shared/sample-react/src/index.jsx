import * as components from "./components";
import React from "react";
import { render } from "react-dom";

window.renderReact = (componentName, containerId) => {
  const Component = components[componentName];
  render(<Component />, document.getElementById(containerId));
};
