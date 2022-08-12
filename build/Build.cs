using System;
using Nuke.Common;

class Build : NukeBuild,
    IRestoreDotNet,

     //IRestoreNuGetCli,
    ICompileDotNet
{
    public static int Main() => Execute<Build>(x => ((ICompile)x).Compile);

    /*Target IRestore.Restore => _ => _
        .Inherit<IRestoreDotNet>();*/

    Target AdditionalLegacyRestore => _ => _
        .DependentFor<ICompile>()
        .Inherit<IRestoreNuGetCli>();
}

interface IRestore : INukeBuild
{
    Target Restore => _ => _
        // Why does the following line make the same restore target running twice?
        .TryDependentFor<ICompile>();
}

interface ICompile : INukeBuild
{
    Target Compile => _ => _
        .TryDependsOn<IRestore>();
}

interface IRestoreNuGetCli : IRestore
{
    Target IRestore.Restore => _ => _
        .Inherit<IRestore>()
        .Executes(() =>
        {
            Console.WriteLine("nuget.exe restore");
        });
}

interface IRestoreDotNet : IRestore
{
    Target IRestore.Restore => _ => _
        .Inherit<IRestore>()
        .Executes(() =>
        {
            Console.WriteLine("dotnet restore");
        });
}

interface ICompileMsBuild : ICompile
{
    Target ICompile.Compile => _ => _
        .Inherit<ICompile>()
        .Executes(() =>
        {
            Console.WriteLine("msbuild.exe sln");
        });
}

interface ICompileDotNet : ICompile
{
    Target ICompile.Compile => _ => _
        .Inherit<ICompile>()
        .Executes(() =>
        {
            Console.WriteLine("dotnet build");
        });
}
