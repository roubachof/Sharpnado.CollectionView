$formsVersion = "3.6.0.220655"

$hlvProject = ".\Sharpnado.HorizontalListView\Sharpnado.HorizontalListView.csproj"
$droidHLVProject = ".\Sharpnado.HorizontalListView.Droid\Sharpnado.HorizontalListView.Droid.csproj"
$iosHLVProject = ".\Sharpnado.HorizontalListView.iOS\Sharpnado.HorizontalListView.iOS.csproj"


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

echo "  building Sharpnado.HorizontalListView solution"
$errorCode = msbuild .\Sharpnado.HorizontalListView.sln /t:Clean,Restore,Build /p:Configuration=Release
if ($errorCode -gt 0)
{
    echo "  Error while building HorizontalListView version, see build.HLV.txt for infos"
    return 5
}


echo "  building Android9 -- only HorizontalListView"
$errorCode = msbuild .\Sharpnado.HorizontalListView.Droid\Sharpnado.HorizontalListView.Droid.csproj /t:Clean,Restore,Build /p:Configuration=ReleaseAndroid9.0
if ($errorCode -gt 0)
{
    echo "  Error while building Android9 HorizontalListView version, see build.Android9.HLV.txt for infos"
    return 6
}


$version = (Get-Item Sharpnado.HorizontalListView\bin\HLVRelease\netstandard2.0\Sharpnado.HorizontalListView.dll).VersionInfo.FileVersion

echo "  packaging Sharpnado.HorizontalListView.nuspec (v$version)"
nuget pack .\Sharpnado.HorizontalListView.nuspec -Version $version
