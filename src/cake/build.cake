#addin "Cake.Powershell"
#r "tools/AddIns/Cake.DotNetCoreVersion/Cake.DotNetCoreVersion.dll"
//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var framework = Argument("framework", "netcoreapp2.0");
var libProject = "./lib/lib.csproj";
var consoleProject = "./console/console.csproj";
var testProject = "./test/test.csproj";

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectory("./lib/bin");
    CleanDirectory("./console/bin");
    CleanDirectory("./test/bin");
});


Task("Restore")
    .IsDependentOn("Clean")
    .Does(() =>
{
    DotNetCoreRestore(libProject);
    DotNetCoreRestore(consoleProject);
    DotNetCoreRestore(testProject);
});

Task("Build")
    .IsDependentOn("Restore")
    .Does(() =>
{
    var settings = new DotNetCoreBuildSettings
    {
        Framework = framework,
        Configuration = configuration,
        OutputDirectory = "./bin/"
    };

    DotNetCoreBuild(libProject, settings);
    DotNetCoreBuild(consoleProject, settings);
    DotNetCoreBuild(testProject, settings);
});


Task("Run_Console")
    .IsDependentOn("Build")
    .Does(() =>
{
    DotNetCoreTool("./console/console.csproj",
            "run");
});

Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
{
    DotNetCoreTool("./test/test.csproj",
            "xunit",  "-xml ./test-results/xunit-results.xml");

    // MA - Transform the result XML into NUnit-compatible XML for the build server.
    XmlTransform("./xunit2nunit.xsl", "./test/test-results/xunit-results.xml", "./test/test-results/nunit-results.xml");
});


//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .IsDependentOn("Build")
    .IsDependentOn("Run_Console")
    .IsDependentOn("Test")
    ;

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);