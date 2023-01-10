# Running the exercise solutions

Before running an exercise solution, you need to set the startup projects. As this is a distributed system, there are 11 projects to run simultaneously.

### Visual Studio

One way to do this is to use the *SwitchStartupProject* Visual Studio extension. After installing the extension, open the exercise solution and choose the "Exercise" startup configuration.

- [SwitchStartupProject](https://marketplace.visualstudio.com/items?itemName=vs-publisher-141975.SwitchStartupProjectForVS2022) for Visual Studio 2022

### Visual Studio without plugin

The startup projects are also listed in the instructions for each exercise. If you need to, you can configure them manually:

- In Visual Studio, right click the solution in the Solution Explorer
- Click "Properties"
- Ensure that, in the left hand pane, "Common Properties", "Start Project" is selected.
- Select the "Multiple startup projects" radio button
- Set the "Action" for each project listed in the instructions for the exercise to "Start".

To run an exercise solution, simply press <kbd>F5</kbd> in Visual Studio. The exercise solution will now be running and fully functional.

### JetBrains Rider

In JetBrains Rider, top-right next to `Run` there's a dropdown that allows you to select the startup project. We added a 'compound' configuration called `Everything` that, once selected, will run all projects.

The startup projects are also listed in the instructions for each exercise. If you need to, you can configure them manually:

- In JetBrains Rider, top-right next to `Run` there's a dropdown that allows you to select the startup project. Click `Edit configuration` in that dropdown.
- In the new dialog window that pops up, click the `+` sign at the top-left.
- Select `Compound` as new configuration.
  - Give it a name
  - Add each project listed in the instructions for the exercise to "Start" to the compound.

To run an exercise solution, simply press <kbd>F5</kbd> in Rider. The exercise solution will now be running and fully functional.

### Note

The solutions contain single page applications (SPAs) and use `IIS Express`. To prevent caching issues, before switching to another exercise:

- Ensure that `IIS Express` is shut down
- Clear the browser cache (or disable it entirely). Alternatively, the cache can cleared by refreshing the page using <kbd>Shift</kbd>+<kbd>F5</kbd> or <kbd>Ctrl</kbd>+<kbd>Shift</kbd>+<kbd>R</kbd> in some browsers.
- When running a solution, the wrong page is sometimes displayed in the browser. Either:
  - Ensure all HTML template files are closed when the application is run, or:
  - Manually change the browser address to the root URL.