@echo off
pushd Story.Ext
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\msbuild /p:Configuration=Release 
popd
nuget pack Story.Ext\Story.Ext.csproj -IncludeReferencedProjects -Prop Configuration=Release
