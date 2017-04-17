var App = (function () {
    function App(elementHost) {
        this.host = elementHost;
    }
    App.prototype.run = function () {
        this.host.innerText = "running from typescript!";
    };
    return App;
}());
//# sourceMappingURL=app.js.map