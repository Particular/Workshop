# How to run samples

* Each sample is configured to be F5 compliant
   * At the end of each `sample documentation` markdown file there is a list of projects that needs to be configured as startup projects
* Samples are configued to use `IIS Express`, please ensure that `IIS Express` is shutdown each time a new sample is started to prevent any caching issue
* SPA runtime behavior, when moving across samples, might be affected by browser caching, please disable the browser cache or clear it when swithing from one sample to the other
   * When running the SPA, if an html template is open in Visual Studio at F5 time it might happen that the wrong home page is displayed:
      * please ensure all html templates are closed when the application is run
      * or manually change the browser address to point to the root url
