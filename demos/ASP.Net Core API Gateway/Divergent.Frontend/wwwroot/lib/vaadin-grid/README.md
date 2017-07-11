![Bower version](https://img.shields.io/bower/v/vaadin-grid.svg)
[![Published on webcomponents.org](https://img.shields.io/badge/webcomponents.org-published-blue.svg)](https://www.webcomponents.org/element/vaadin/vaadin-grid)
[![Build Status](https://travis-ci.org/vaadin/vaadin-grid.svg?branch=master)](https://travis-ci.org/vaadin/vaadin-grid)
[![Coverage Status](https://coveralls.io/repos/github/vaadin/vaadin-grid/badge.svg?branch=master)](https://coveralls.io/github/vaadin/vaadin-grid?branch=master)
[![Gitter](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/vaadin/vaadin-core-elements?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge)

# &lt;vaadin-grid&gt; v2

[Live Demo ↗](https://cdn.vaadin.com/vaadin-grid/2.0.2/demo/)

> :eyes: Looking for &lt;vaadin-grid&gt; v1.x? Please see the [the v1 branch](https://github.com/vaadin/vaadin-grid/tree/1.x)

[&lt;vaadin-grid&gt;](https://vaadin.com/elements/-/element/vaadin-grid) is a free, high quality data grid / data table [Polymer](http://polymer-project.org) element, part of the [Vaadin Core Elements](https://vaadin.com/elements).

<!---
```
<custom-element-demo>
  <template>
    <script src="../webcomponentsjs/webcomponents-lite.js"></script>
    <link rel="import" href="vaadin-grid.html">
    <next-code-block></next-code-block>
  </template>
</custom-element-demo>
```
-->
```html
<template is="dom-bind">
  <x-data-provider data-provider="{{dataProvider}}"></x-data-provider>

  <vaadin-grid data-provider="[[dataProvider]]" size="200">

    <vaadin-grid-column width="50px" flex-grow="0">
      <template class="header">#</template>
      <template>[[index]]</template>
    </vaadin-grid-column>

    <vaadin-grid-column width="50px" flex-grow="0">
      <template class="header"></template>
      <template>
        <iron-image src="[[item.picture.thumbnail]]"></iron-image>
      </template>
    </vaadin-grid-column>

    <vaadin-grid-column width="calc(50% - 100px)">
      <template class="header">First Name</template>
      <template>[[item.name.first]]</template>
    </vaadin-grid-column>

    <vaadin-grid-column width="calc(50% - 100px)">
      <template class="header">Last Name</template>
      <template>[[item.name.last]]</template>
    </vaadin-grid-column>

  </vaadin-grid>
</template>
```

<img src="https://github.com/vaadin/vaadin-grid/raw/master/grid.gif">


## Contributing

1. Fork the `vaadin-grid` repository and clone it locally.

1. Make sure you have [npm](https://www.npmjs.com/) installed.

1. When in the `vaadin-grid` directory, run `npm install` to install dependencies.


## Running demos and tests in browser

1. Install [polyserve](https://www.npmjs.com/package/polyserve): `npm install -g polyserve`

1. When in the `vaadin-grid` directory, run `polyserve --open`, browser will automatically open the component API documentation.

1. You can also open demo or in-browser tests by adding **demo** or **test** to the URL, for example:

  - http://127.0.0.1:8080/components/vaadin-grid/demo
  - http://127.0.0.1:8080/components/vaadin-grid/test


## Running tests from the command line

1. Install [web-component-tester](https://www.npmjs.com/package/web-component-tester): `npm install -g web-component-tester`

1. When in the `vaadin-grid` directory, run `wct` or `npm test`


## Following the coding style

We are using [ESLint](http://eslint.org/) for linting JavaScript code. You can check if your code is following our standards by running `gulp lint`, which will automatically lint all `.js` files as well as JavaScript snippets inside `.html` files.


## Creating a pull request

  - Make sure your code is compliant with our code linters: `gulp lint`
  - Check that tests are passing: `npm test`
  - [Submit a pull request](https://www.digitalocean.com/community/tutorials/how-to-create-a-pull-request-on-github) with detailed title and description
  - Wait for response from one of Vaadin Elements team members


## License

Apache License 2.0
