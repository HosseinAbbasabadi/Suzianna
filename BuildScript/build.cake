#tool "nuget:?package=xunit.runner.console"

var solutionPath = Argument("SolutionPath", "../Code/Suzianna.sln");
var buildNumber = Argument("BuildNumber","0");

Task("Clean")
    .Does(()=>{
        MSBuild(solutionPath, configurator =>
            configurator.SetConfiguration("Release")
                .SetVerbosity(Verbosity.Minimal)
                .WithTarget("Clean")
                .UseToolVersion(MSBuildToolVersion.VS2017)
                .SetMSBuildPlatform(MSBuildPlatform.x86)
                .SetPlatformTarget(PlatformTarget.MSIL));
});

Task("Restore-NuGet-Packages")
    .Does(() =>
{
    NuGetRestore(solutionPath);
});

Task("Build")
.Does(()=>
{
    MSBuild(solutionPath, configurator =>
        configurator.SetConfiguration("Release")
            .SetVerbosity(Verbosity.Minimal)
            .UseToolVersion(MSBuildToolVersion.VS2017)
            .SetMSBuildPlatform(MSBuildPlatform.x86)
            .SetPlatformTarget(PlatformTarget.MSIL));
});

Task("Run-Unit-Tests")
    .Does(() =>
{
    var testAssemblies = GetFiles("../Code/**/bin/Release/*.Tests.Unit.dll");
    XUnit2(testAssemblies,
        new XUnit2Settings {
        Parallelism = ParallelismOption.All,
        NoAppDomain = true,
    });
});

Task("Default")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore-NuGet-Packages")
    .IsDependentOn("Build")
    .IsDependentOn("Run-Unit-Tests")
   ;

RunTarget("Default");