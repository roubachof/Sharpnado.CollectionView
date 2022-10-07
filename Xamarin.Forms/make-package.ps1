$formsVersion = "4.5.0.356"

$hlvProject = ".\Sharpnado.CollectionView\Sharpnado.CollectionView.csproj"
$droidHLVProject = ".\Sharpnado.CollectionView.Droid\Sharpnado.CollectionView.Droid.csproj"
$iosHLVProject = ".\Sharpnado.CollectionView.iOS\Sharpnado.CollectionView.iOS.csproj"

$droidBin = ".\Sharpnado.CollectionView.Droid\bin\Release"
$droidObj = ".\Sharpnado.CollectionView.Droid\obj\Release"

rm *.txt

echo "  Setting Xamarin.Forms version to $formsVersion"

$findXFVersion = '(Xamarin.Forms">\s+<Version>)(.+)(<\/Version>)'
$replaceString = "`$1 $formsVersion `$3"

(Get-Content $hlvProject -Raw)  -replace $findXFVersion, "$replaceString" | Out-File $hlvProject
(Get-Content $droidHLVProject -Raw)  -replace $findXFVersion, "$replaceString" | Out-File $droidHLVProject
(Get-Content $iosHLVProject -Raw)  -replace $findXFVersion, "$replaceString" | Out-File $iosHLVProject


echo "########################################"
echo "# Sharpnado.CollectionView"
echo "########################################"

echo "  deleting android bin-obj folders"
rm -Force -Recurse $droidBin
rm -Force -Recurse $droidObj

echo "  cleaning Sharpnado.CollectionView solution"
msbuild .\Sharpnado.CollectionView.sln /t:Clean

if ($LastExitCode -gt 0)
{
    echo "  Error while cleaning solution"
    return
}

echo "  restoring Sharpnado.CollectionView solution packages"
msbuild .\Sharpnado.CollectionView.sln /t:Restore

if ($LastExitCode -gt 0)
{
    echo "  Error while restoring packages"
    return
}

echo "  building Sharpnado.CollectionView solution"
msbuild .\Sharpnado.CollectionView.sln /t:Build /p:Configuration=Release
if ($LastExitCode -gt 0)
{
    echo "  Error while building solution"
    return
}


echo "###############################################"
echo "# Packaging"
echo "###############################################"

$version = (Get-Item Sharpnado.CollectionView\bin\Release\netstandard2.0\Sharpnado.CollectionView.dll).VersionInfo.FileVersion

echo "  packaging Sharpnado.CollectionView.nuspec (v$version)"
nuget pack .\Sharpnado.CollectionView.nuspec -Version $version
