class App
{
    host: HTMLElement;

    public constructor(elementHost: HTMLElement) {
        this.host = elementHost;
    }

    run() {
        this.host.innerText = "running from typescript!";
    }
}