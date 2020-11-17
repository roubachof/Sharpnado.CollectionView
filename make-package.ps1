$formsVersion = "3.6.0.220655"

$hlvProject = ".\Sharpnado.HorizontalListView\Sharpnado.HorizontalListView.csproj"
$droidHLVProject = ".\Sharpnado.HorizontalListView.Droid\Sharpnado.HorizontalListView.Droid.csproj"
$iosHLVProject = ".\Sharpnado.HorizontalListView.iOS\Sharpnado.HorizontalListView.iOS.csproj"

$droidBin = ".\Sharpnado.HorizontalListView.Droid\bin\Release"
$droidObj = ".\Sharpnado.HorizontalListView.Droid\obj\Release"

rm *.txt

echo "  Setting Xamarin.Forms version to $formsVersion"

$findXFVersion = '(Xamarin.Forms">\s+<Version>)(.+)(<\/Version>)'
$replaceString = "`$1 $formsVersion `$3"

(Get-Content $hlvProject -Raw)  -replace $findXFVersion, "$replaceString" | Out-File $hlvProject
(Get-Content $droidHLVProject -Raw)  -replace $findXFVersion, "$replaceString" | Out-File $droidHLVProject
(Get-Content $iosHLVProject -Raw)  -replace $findXFVersion, "$replaceString" | Out-File $iosHLVProject


echo "########################################"
echo "# Sharpnado.Forms.HorizontalListView"
echo "########################################"

echo "  deleting android bin-obj folders"
rm -Force -Recurse $droidBin
rm -Force -Recurse $droidObj

echo "  cleaning Sharpnado.HorizontalListView solution"
msbuild .\Sharpnado.HorizontalListView.sln /t:Clean

if ($LastExitCode -gt 0)
{
    echo "  Error while cleaning solution"
    return
}

echo "  restoring Sharpnado.HorizontalListView solution packages"
msbuild .\Sharpnado.HorizontalListView.sln /t:Restore

if ($LastExitCode -gt 0)
{
    echo "  Error while restoring packages"
    return
}

echo "  building Sharpnado.HorizontalListView solution"
msbuild .\Sharpnado.HorizontalListView.sln /t:Build /p:Configuration=Release
if ($LastExitCode -gt 0)
{
    echo "  Error while building solution"
    return
}


echo "###############################################"
echo "# Android 9.0 and below"
echo "###############################################"

echo "  deleting android obj folders"
rm -Force -Recurse $droidObj
if ($LastExitCode -gt 0)
{
    echo "  Error deleting android obj folder"
    return
}

echo "  cleaning Android9 solution"
msbuild .\Sharpnado.HorizontalListView.Droid\Sharpnado.HorizontalListView.Droid.csproj /t:Clean /p:Configuration=ReleaseAndroid9.0

if ($LastExitCode -gt 0)
{
    echo "  Error while cleaning solution"
    return
}

echo "  restoring Android9 solution packages"
msbuild .\Sharpnado.HorizontalListView.Droid\Sharpnado.HorizontalListView.Droid.csproj /t:Restore /p:Configuration=ReleaseAndroid9.0

if ($LastExitCode -gt 0)
{
    echo "  Error while restoring packages"
    return
}

echo "  building Android9"
msbuild .\Sharpnado.HorizontalListView.Droid\Sharpnado.HorizontalListView.Droid.csproj /t:Build /p:Configuration=ReleaseAndroid9.0
if ($LastExitCode -gt 0)
{
    echo "  Error while building solution for Android9"
    return
}


echo "###############################################"
echo "# Packaging"
echo "###############################################"

$version = (Get-Item Sharpnado.HorizontalListView\bin\Release\netstandard2.0\Sharpnado.HorizontalListView.dll).VersionInfo.FileVersion

echo "  packaging Sharpnado.HorizontalListView.nuspec (v$version)"
nuget pack .\Sharpnado.HorizontalListView.nuspec -Version $version
