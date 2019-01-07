$formsVersion = "2.5.1.444934"

$netstandardProject = ".\Sharpnado.Presentation.Forms\Sharpnado.Presentation.Forms.csproj"
$droidProject = ".\Sharpnado.Presentation.Forms.Droid\Sharpnado.Presentation.Forms.Droid.csproj"
$iosProject = ".\Sharpnado.Presentation.Forms.iOS\Sharpnado.Presentation.Forms.iOS.csproj"

echo "  Setting Xamarin.Forms version to $formsVersion"

$findXFVersion = '(Xamarin.Forms">\s+<Version>)(.+)(</Version>)'
$replaceString = "`$1 $formsVersion `$3"

(Get-Content $netstandardProject -Raw)  -replace $findXFVersion, "$replaceString" | Out-File $netstandardProject
(Get-Content $droidProject -Raw)  -replace $findXFVersion, "$replaceString" | Out-File $droidProject
(Get-Content $iosProject -Raw)  -replace $findXFVersion, "$replaceString" | Out-File $iosProject

echo "  building Sharpnado.Presentation.Forms solution -- normal mode"
msbuild .\Sharpnado.Presentation.Forms.sln /t:Clean,Restore,Build /p:Configuration=Release > build.txt
echo "  packaging Sharpnado.Presentation.Forms.nuspec"
nuget pack .\Sharpnado.Presentation.Forms.nuspec > $null

echo "  backuping Sharpnado.Presentation.Forms.csproj"
cp Sharpnado.Presentation.Forms\Sharpnado.Presentation.Forms.csproj Sharpnado.Presentation.Forms\Sharpnado.Presentation.Forms.csproj.bak
echo "  renaming Sharpnado.Presentation.Forms.HorizontalListView.csproj to Sharpnado.Presentation.Forms.csproj"
cp Sharpnado.Presentation.Forms\Sharpnado.Presentation.Forms.HorizontalListView.csproj Sharpnado.Presentation.Forms\Sharpnado.Presentation.Forms.csproj 

echo "  building Sharpnado.Presentation.Forms solution -- only HorizontalListView"
msbuild .\Sharpnado.Presentation.Forms.sln /t:Clean,Restore,Build /p:Configuration=Release > build.HorizontalListView.txt
echo "  packaging Sharpnado.Forms.HorizontalListView.nuspec"
nuget pack .\Sharpnado.Forms.HorizontalListView.nuspec > $null

echo "  restoring Sharpnado.Presentation.Forms.csproj"
cp Sharpnado.Presentation.Forms\Sharpnado.Presentation.Forms.csproj.bak Sharpnado.Presentation.Forms\Sharpnado.Presentation.Forms.csproj
rm Sharpnado.Presentation.Forms\Sharpnado.Presentation.Forms.csproj.bak