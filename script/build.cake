var target = Argument("target", "Publish");
var configuration = Argument("configuration", "Release");
#addin nuget:?package=Cake.Git

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

var basePath = "C:/Users/azaza/source/repos";

Task("Git-Pull")
.Does((context) =>
{

    foreach (var path in new[]{basePath + "/a",basePath + "/b",basePath + "/c"})
    {
        context.Environment.WorkingDirectory = path;
        StartProcess("C:/Program Files/Git/bin/git.exe", "pull");
    }
});

Task("A")
    .Does(() =>
{

     DotNetBuild(basePath + "/A/A.sln", new DotNetBuildSettings
    {
        Configuration = configuration,
    });
});
 Task("B")
    .Does(() =>
{
    CopyFile(basePath+"/A/ClassLibrary1/obj/Release/net6.0/ClassLibrary1.dll",basePath + "/B/B/common_libs/ClassLibrary1.dll");
     DotNetBuild(basePath + "/B/B.sln", new DotNetBuildSettings
    {
        Configuration = configuration,
    });
});
 Task("C")
 
    .Does(() =>
{
    CopyFile(basePath + "/A/ClassLibrary1/obj/Release/net6.0/ClassLibrary1.dll",basePath + "/C/C/common_libs/ClassLibrary1.dll");
    CopyFile(basePath + "/B/ClassLibrary1/obj/Release/net6.0/ClassLibrary2.dll", basePath + "/C/C/common_libs/ClassLibrary2.dll");
     DotNetBuild(basePath + "/C/C.sln", new DotNetBuildSettings
    {
        Configuration = configuration,
    });
});

Task("Test")
    .IsDependentOn("Git-Pull")
    .IsDependentOn("TestA")
    .IsDependentOn("TestB")
    .IsDependentOn("TestC")
    
    .Does(() =>
{   
});

Task("TestA")
    .IsDependentOn("A")
    .Does(() =>
{
    
    DotNetTest(basePath + "/A/TestProject1/obj/Release/net6.0/TestProject1.dll", new DotNetTestSettings
    {
        Configuration = configuration,
        NoBuild = false,
    });
});

Task("TestB")
    .IsDependentOn("B")
    .Does(() =>
{
    DotNetTest(basePath + "/B/TestProject1/obj/Release/net6.0/TestProject1.dll", new DotNetTestSettings
    {
        Configuration = configuration,
        NoBuild = false,
    });
});

Task("TestC")
    .IsDependentOn("C")
    .Does(() =>
{
    DotNetTest(basePath +"/C/TestProject1/obj/Release/net6.0/TestProject1.dll", new DotNetTestSettings
    {
        Configuration = configuration,
        NoBuild = false,
    });
});

Task("Publish")
    .IsDependentOn("Test")
    .Does(() =>  
{
  var pathToSaveA =  @basePath + "/safe/A";
  var pathToSaveB =  @basePath + "/safe/B";
  var pathToSaveC =  @basePath + "/safe/C";

  CopyFiles(@basePath + @"/A/**/*", pathToSaveA);
  CopyFiles(@basePath + @"/B/**/*", pathToSaveB);
  CopyFiles(@basePath + @"/C/**/*", pathToSaveC);
});

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);