# How to run the exercise solutions

- Each exercise solution is configured to be running and fully functional just by pressing <kbd>F5</kbd> in Visual Studio.
  - In case you have problems, the projects that need to be configured as startup projects are listed in the instructions for each exercise.
- The solutions contain single page applications (SPAs) and use `IIS Express`. To prevent caching issues, before switching to another exercise:
  - Ensure that `IIS Express` is shut down
  - Clear the browser cache (or disable it entirely). Alternatively, the cache can cleared by refreshing the page using <kbd>Ctrl</kbd>+<kbd>F5</kbd> in some browsers.
- When running a solution, the wrong page is sometimes displayed in the browser. Either:
  - Ensure all HTML template files are closed when the application is run, or:
  - Manually change the browser address to the root URL.
