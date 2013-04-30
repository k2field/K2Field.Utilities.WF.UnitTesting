===============================
So what is it?
===============================

It is a way of using configured xml files to automate testing of K2 Process Instances, including actioning a task as another user and IPCs. It has a UI which logs each activity (taken or not taken), displays new K2 errors, and can test the values of K2 process/activity datafields. As a bonus it can also display the viewflow in a new tab in the UI.

===============================
So can I see it in action?
===============================

Yes. I have a brief video with no sound at http://screencast.com/t/dpQvRYHo.
Also the project has an [example workflow](/Testing/SampleWorkflow/K2Field.Utilities.Testing.SampleWorkflow.sln) 
Steps to get this working fast:
•	Open up the Sample workflow solution on github and deploy
•	Open up the [Workflow Testing Solution](/Testing/WF/K2Field.Utilities.Testing.WF.sln), set the UI project to be the start-up project and run it.
•	Change the Viewflow URL and K2 server settings to match your environment and click “Start Tests”
![Example Screenshot](/SampleScreenshot1.png)
 
===============================
Isn’t this already on K2Underground?
===============================

Yes it’s based on Adam Castle’s excellent [k2underground project](http://www.k2underground.com/groups/workflow_testing_tools/default.aspx). I’ve revamped the xml schema and added new features. I’ve also forked off his helper project, so it is a little bit difficult to merge back in. We will work on it. The old xml schema should still work.

===============================
Is this documented?
===============================

A bit. The example should give you enough examples to get going with. I’ve tried to keep [the documentation](/WorkflowTesting.docx) up to date with all the new features I’ve added. 

===============================
And what are these new features?
===============================

Mainly the ability to re-use process/activity files. The example solution has a Travel Process and an Advanced Approval Process that has its own set of tests. These are then called from the Travel process tests.
Also
•	Check blackpearl’s error log count and show new errors
•	Check that paths are taken
•	Check that paths are not taken

===============================
So where has this been used?
===============================

I use it all the time on any K2 Process project. It means I never have to check the K2 workspace or process portals. It works best when workflows are extremely de-coupled (see below), but you can also hook in any call to a .Net assembly either before or after every activity.
I used it pretty extensively at a Lloyd's syndicate. They had 12+ processes all IPC dependant on each other and the most insane complexity I’ve ever seen in a workflow app, as the whole business process could be started at 3 different places and the 3 main paths all had to be taken to complete the process. **It would not have been possible to completely test all paths without this tool.** The links between 50 xml files got so complex I had to visualise them in Google docs:
![Example complexity](/ComplexXMLstructure.png)

=============================== 
Questions?
===============================

As normal, use github to request features and email me if you are stuck.
