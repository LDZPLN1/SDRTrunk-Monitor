SDRTrunk Monitor is a Windows only tray application to manage the SDRTrunk process and auto-restart it under certain application failure modes.

This app will only monitor SDRTrunk processes spawned by this app, e.g. if you started SDRTrunk from the .bat file, you will not be able to monitor it or auto-restart on errors.

The first time you run it, or if the SDRTrunk path is not known, you will be prompted to open the settings dialog.

If the app detects another instance of SDRTrunk is already running, the Start option will not be available.

Settings Overview:
* SDRTrunk Path - This should point to the base path for SDRTrunk (the folder that holds the bin, conf, legal, lib folders, etc.)
* SDRTrunk Version - Select the version you are running. This setting affects the parameters passed to the Java executable
* External Command - (Optional) An external command that can be run in between restarts of SDRTrunk. Enclose any options that have spaces in quotes
* Timed Command - (Optional) An externa command to run every "x" seconds while SDRTrunk is running
* Poll Timer - How often the app polls the SDRTrunk process to verify it is still running.
* Ext Timer - How often to run the Timed Command

Toggle Menu Options:
* Auto Restart - Force an automated restart of SDRTrunk if the process dies or an error is detected. If this option is not checked, you will only receive a single tray notification when an error appears to have occurred or the process appears to have died
* External Command - Execute the external command specified in the Settings dialog in between restarts of SDRTrunk. It will not run between manual stop/starts. If no external command has been set in the Settings dialog, this option will not be available. (In my use case,  this fires a Python script to update the SDRTrunk playlist.xml file with newly identified radio IDs from a discovery database before it restarts)

I'm sure the code could be cleaner... but I do this for fun and in my spare time.

Only tested on Windows 11, but it should work on Windows 10 as well.

As an example, I use the external command setting to execute a Python script to update the SDRTrunk XML file in between restarts with new radio ID data from a database. The timed command is used to execute a Python script that imports SDRTrunk data once per hour and updates the database.  This script also scans the saved MP3 files to perform audio detection (to discard dead air files) and archives files into a folder structure based on date/system/group/TGID for known RIDs. This leaves me with a short list of RIDs to identify.