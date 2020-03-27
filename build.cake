#tool "nuget:https://www.nuget.org/api/v2/?package=OctopusTools"
//////////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////////////////
var target = Argument("target", "Default");
var configuration = Argument("configuration", "Keystone");
var releaseNumber = Argument("releaseNumber", "0.0.1");
var deployRelease = Argument("deployRelease", "False");
var releaseNotes = Argument("releaseNotes", "");
var apiKey = Argument("apiKey", ""); //API-GMDHDJMIZIVSI3REFKED1FAKSAA
var server = Argument("server", ""); //http://192.168.14.109/Octopus2.0

//////////////////////////////////////////////////////////////////////////////////
//	PREPARATION
//////////////////////////////////////////////////////////////////////////////////
var binDir = Directory("./bin");
var buildDir = binDir + Directory(configuration);
var isDeployment = bool.Parse(deployRelease);
var isGSABuild = configuration.ToUpper().Equals("PRODUCTIONGSA");

var reportServerInstallerDir = Directory("./iBank.ReportServer.Installer/bin") + Directory(configuration);
var bcstServerInstallerDir = Directory("./iBank.BroadcastServer.Installer/bin") + Directory(configuration);
var queueMgrInstallerDir = Directory("./iBank.BroadcastServer.QueueManager.Installer/bin") + Directory(configuration);
var overdueInstallerDir = Directory("./iBank.OverdueBroadcastMonitor.Installer/bin") + Directory(configuration);
var reportQueueMgrInstallerDir = Directory("./iBank.ReportQueueManager.Installer/bin") + Directory(configuration);

//////////////////////////////////////////////////////////////////////////////////
//	TASKS
//////////////////////////////////////////////////////////////////////////////////
Task("Clean")
.Does(() => {
	CleanDirectory(buildDir);
	CleanDirectory(reportServerInstallerDir);
	CleanDirectory(bcstServerInstallerDir);
	CleanDirectory(queueMgrInstallerDir);
	CleanDirectory(overdueInstallerDir);
	CleanDirectory(reportQueueMgrInstallerDir);
});

Task("Restore-Nuget-Packages")
.IsDependentOn("Clean")
.Does(() => {
	NuGetRestore("iBank.Reporting.sln");
	DotNetCoreRestore();
});

Task("Update-Version")
.IsDependentOn("Restore-Nuget-Packages")
.Does(() => {
	var versionSettings = new AssemblyInfoSettings {
		Company = "Cornerstone Information Systems",
		Copyright = string.Format("Copyright (c) Cornerstone Information Systems {0}", DateTime.Now.Year),
		ComVisible = false,
		Version = releaseNumber,
		FileVersion = releaseNumber
	};

	//broadcast Server
	var file = "./iBank.BroadcastServer/Properties/AssemblyInfo.cs";
	var filePath = MakeAbsolute(File(file));
	var fileInfo = new System.IO.FileInfo(filePath.FullPath);
	fileInfo.IsReadOnly = false;

	var parsedInfo = ParseAssemblyInfo(file);
	versionSettings.Title = parsedInfo.Title;
	versionSettings.Product = parsedInfo.Product;
	versionSettings.Guid = parsedInfo.Guid;
	CreateAssemblyInfo(file, versionSettings);

	//broadcast queue manager
	file = "./iBank.BroadcastServer.QueueManager/Properties/AssemblyInfo.cs";
	filePath = MakeAbsolute(File(file));
	fileInfo = new System.IO.FileInfo(filePath.FullPath);
	fileInfo.IsReadOnly = false;

	parsedInfo = ParseAssemblyInfo(file);
	versionSettings.Title = parsedInfo.Title;
	versionSettings.Product = parsedInfo.Product;
	versionSettings.Guid = parsedInfo.Guid;
	CreateAssemblyInfo(file, versionSettings);

	//report Server
	file = "./iBank.Client/Properties/AssemblyInfo.cs";
	filePath = MakeAbsolute(File(file));
	fileInfo = new System.IO.FileInfo(filePath.FullPath);
	fileInfo.IsReadOnly = false;

	parsedInfo = ParseAssemblyInfo(file);
	versionSettings.Title = parsedInfo.Title;
	versionSettings.Product = parsedInfo.Product;
	versionSettings.Guid = parsedInfo.Guid;
	CreateAssemblyInfo(file, versionSettings);

	//overdue broadcast monitor
	file = "./iBank.OverdueBroadcastMonitor/Properties/AssemblyInfo.cs";
	filePath = MakeAbsolute(File(file));
	fileInfo = new System.IO.FileInfo(filePath.FullPath);
	fileInfo.IsReadOnly = false;

	parsedInfo = ParseAssemblyInfo(file);
	versionSettings.Title = parsedInfo.Title;
	versionSettings.Product = parsedInfo.Product;
	versionSettings.Guid = parsedInfo.Guid;
	CreateAssemblyInfo(file, versionSettings);

	//report queue manager
	file = "./iBank.ReportQueueManager/Properties/AssemblyInfo.cs";
	filePath = MakeAbsolute(File(file));
	fileInfo = new System.IO.FileInfo(filePath.FullPath);
	fileInfo.IsReadOnly = false;

	parsedInfo = ParseAssemblyInfo(file);
	versionSettings.Title = parsedInfo.Title;
	versionSettings.Product = parsedInfo.Product;
	versionSettings.Guid = parsedInfo.Guid;
	CreateAssemblyInfo(file, versionSettings);
});

var buildSettings = new MSBuildSettings()
						.SetConfiguration(configuration)
						.SetMSBuildPlatform(MSBuildPlatform.x64)
						.SetPlatformTarget(PlatformTarget.x64)
						.UseToolVersion(MSBuildToolVersion.VS2017);

Task("Build")
	.IsDependentOn("Update-Version")
	.Does(() => {
		MSBuild(File("iBank.Reporting.sln"), buildSettings);
	});

Task("Run-Tests")
	.IsDependentOn("Build")
	.Does(() => {
		var testDllPath = "./iBank.UnitTesting/bin/" + configuration + "/iBank.UnitTesting.dll";
		var resultsFileName = "iBankUnitTestResults.trx";

		if(FileExists(resultsFileName))
		{
			Information("Deleting previous test results file.");
			DeleteFiles(resultsFileName);
		}
		
		VSTest(testDllPath, new VSTestSettings { PlatformArchitecture = VSTestPlatform.x64 } );
	});

Task("Build-Installers")
	.IsDependentOn("Run-Tests")
	.Does(() => {
		MSBuild("./iBank.ReportServer.Installer/iBank.ReportServer.Installer.wixproj", buildSettings);

		MSBuild("./iBank.BroadcastServer.Installer/iBank.BroadcastServer.Installer.wixproj", buildSettings);

		MSBuild("./iBank.BroadcastServer.QueueManager.Installer/iBank.BroadcastServer.QueueManager.Installer.wixproj", buildSettings);

		MSBuild("./iBank.OverdueBroadcastMonitor.Installer/iBank.OverdueBroadcastMonitor.Installer.wixproj", buildSettings);

		MSBuild("./iBank.ReportQueueManager.Installer/iBank.ReportQueueManager.Installer.wixproj", buildSettings);
	});

var nugetOutputDir = Directory("C:/CISNugetInstalls");
var formattablePackagePath = "C:/CISNugetInstalls/{0}." + releaseNumber + ".nupkg";

var packSettings = new OctopusPackSettings {
										OutFolder = nugetOutputDir,
										Version = releaseNumber,
										Overwrite = true,
										ReleaseNotes = releaseNotes,
										BasePath = reportServerInstallerDir
									};
var pushSettings = new OctopusPushSettings { ReplaceExisting = true };

var releaseSettings = new CreateReleaseSettings {
								Server = server,
								ApiKey = apiKey,
								ReleaseNumber = releaseNumber,
								ReleaseNotes = releaseNotes,
								DeployTo = isDeployment ? "Keystone Stage" : null
						};

var packageId = "";

//report server
Task("Stage-Report-Server-Release")
	.Does(() => {
		packSettings.BasePath = reportServerInstallerDir;
		packageId = isGSABuild ? "ciswired.iBank.NET.ReportServer.GSA" : "ciswired.iBank.NET.ReportServer";
		OctoPack(packageId, packSettings);

		var reportServerPackagePath = string.Format(formattablePackagePath, packageId);
		OctoPush(server, apiKey, new FilePath(reportServerPackagePath), pushSettings);

		if(FileExists(reportServerPackagePath)) DeleteFile(reportServerPackagePath);
	});

Task("Create-Report-Server-Release")
	.Does(() => {
		OctoCreateRelease("iBank Report Server", releaseSettings);
	});

//broadcast server
Task("Stage-Broadcast-Server-Release")
	.Does(() => {
		packSettings.BasePath = bcstServerInstallerDir;
		packageId = isGSABuild ? "ciswired.iBank.NET.BroadcastServer.GSA" : "ciswired.iBank.NET.BroadcastServer";
		OctoPack(packageId, packSettings);

		var bcstServerPackagePath = string.Format(formattablePackagePath, packageId);
		OctoPush(server, apiKey, new FilePath(bcstServerPackagePath), pushSettings);

		if(FileExists(bcstServerPackagePath)) DeleteFile(bcstServerPackagePath);
	});

Task("Create-Broadcast-Server-Release")
	.Does(() => {
		OctoCreateRelease("iBank Broadcast Server", releaseSettings);
	});

//queue manager
Task("Stage-Queue-Mgr-Release")
	.Does(() => {
		packSettings.BasePath = queueMgrInstallerDir;
		packageId = isGSABuild ? "ciswired.BroadcastQueueManager.GSA" : "ciswired.BroadcastQueueManager";
		OctoPack(packageId, packSettings);

		var queueMgrPackagePath = string.Format(formattablePackagePath, packageId);
		OctoPush(server, apiKey, new FilePath(queueMgrPackagePath), pushSettings);

		if(FileExists(queueMgrPackagePath)) DeleteFile(queueMgrPackagePath);
	});

Task("Create-Queue-Mgr-Release")
	.Does(() => {
		OctoCreateRelease("iBank Broadcast Queue Manager", releaseSettings);
	});

//overdue broadcast monitor
Task("Stage-Overdue-Broadcast-Monitor-Release")
	.Does(() => {
		packSettings.BasePath = overdueInstallerDir;
		packageId = isGSABuild ? "ciswired.iBank.NET.OverdueBroadcastMonitor.GSA" : "ciswired.iBank.NET.OverdueBroadcastMonitor";
		OctoPack(packageId, packSettings);

		var overduePackagePath = string.Format(formattablePackagePath, packageId);
		OctoPush(server, apiKey, new FilePath(overduePackagePath), pushSettings);

		if(FileExists(overduePackagePath)) DeleteFile(overduePackagePath);
	});

Task("Create-Overdue-Broadcast-Monitor-Release")
	.Does(() => {
		OctoCreateRelease("iBank Overdue Broadcast Monitor", releaseSettings);
	});

//report queue manager
Task("Stage-Report-Queue-Manager-Release")
	.Does(() => {
		packSettings.BasePath = reportQueueMgrInstallerDir;
		packageId = isGSABuild ? "ciswired.iBank.NET.ReportQueueManager.GSA" : "ciswired.iBank.NET.ReportQueueManager";
		OctoPack(packageId, packSettings);

		var reportQueueMgrPath = string.Format(formattablePackagePath, packageId);
		OctoPush(server, apiKey, new FilePath(reportQueueMgrPath), pushSettings);

		if(FileExists(reportQueueMgrPath)) DeleteFile(reportQueueMgrPath);
	});

Task("Create-Report-Queue-Manager-Release")
	.Does(() => {
		OctoCreateRelease("iBank Report Queue Manager", releaseSettings);
	});


//crystal reports
Task("Stage-Crystal-Reports-Release")
	.Does(() => {
		packSettings.BasePath = Directory("./iBank.Services.Implementation/CrystalReports");
		packSettings.Format = OctopusPackFormat.Zip;
		packageId = "ciswired.iBank.NET.CrystalReports";
		OctoPack(packageId, packSettings);

		var crystalPackagePath = string.Format(formattablePackagePath, packageId).Replace("nupkg", "zip");
		OctoPush(server, apiKey, new FilePath(crystalPackagePath), pushSettings);

		if(FileExists(crystalPackagePath)) DeleteFile(crystalPackagePath);
	});

Task("Create-Crystal-Reports-Release")
	.Does(() => {
		OctoCreateRelease("iBank Crystal Reports", releaseSettings);
	});

//////////////////////////////////////////////////////////////////////////////////
//	TASK TARGETS
//////////////////////////////////////////////////////////////////////////////////
Task("Default")
	.IsDependentOn("Build-Installers");

Task("Stage-Releases")
	.IsDependentOn("Build-Installers")
	.IsDependentOn("Stage-Report-Server-Release")
	.IsDependentOn("Stage-Broadcast-Server-Release")
	.IsDependentOn("Stage-Queue-Mgr-Release")
	.IsDependentOn("Stage-Overdue-Broadcast-Monitor-Release")
	.IsDependentOn("Stage-Report-Queue-Manager-Release")
	.IsDependentOn("Stage-Crystal-Reports-Release");
	

Task("Create-Releases")
	.IsDependentOn("Create-Report-Server-Release")
	.IsDependentOn("Create-Broadcast-Server-Release")
	.IsDependentOn("Create-Queue-Mgr-Release")
	.IsDependentOn("Create-Overdue-Broadcast-Monitor-Release")
	.IsDependentOn("Create-Report-Queue-Manager-Release")
	.IsDependentOn("Create-Crystal-Reports-Release");

Task("Release-All")
	.IsDependentOn("Stage-Releases")
	.IsDependentOn("Create-Releases");

Task("Release-Report-Queue-Manager")
	.IsDependentOn("Stage-Report-Queue-Manager-Release")
	.IsDependentOn("Create-Report-Queue-Manager-Release");

//////////////////////////////////////////////////////////////////////////////////
//	EXECUTION
//////////////////////////////////////////////////////////////////////////////////
RunTarget(target);