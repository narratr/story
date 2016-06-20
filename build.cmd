@echo off

nuget install Story.Core\packages.config -o packages

pushd Story.Core
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\msbuild /p:Configuration=Release 
popd

pushd Story.Ext
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\msbuild /p:Configuration=Release 
popd

nuget pack Story.Core\Story.Core.csproj -IncludeReferencedProjects -Prop Configuration=Release
nuget pack Story.Ext\Story.Ext.csproj -IncludeReferencedProjects -Prop Configuration=Release
