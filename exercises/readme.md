# How to run samples

* Each sample is configured to be fully functional and running after just pressing the `F5` button
   * At the end of each markdown file describing the exercise, there is a list of projects that need to be configured as startup projects.
* Samples are configured to use `IIS Express`. Ensure that `IIS Express` is shut down every time a new sample is started to prevent caching issues.
* The sample runtime behavior might be affected by browser caching when switching between exercises, because it's a SPA application. Disable the browser cache or clear it when switching from one sample to another.
   * When running the exercise sometimes the wrong home page is displayed. In such case:
      * ensure all HTML template files are closed when the application is run
      * or manually change the browser address to point to the root url.
