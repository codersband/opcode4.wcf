/// <binding AfterBuild='nuget-pack' />

var gulp = require("gulp"),
    nuget = require("gulp-nuget");

var projectName = 'opcode4.wcf';
var apiKey = '75af7863-f210-49f5-b26e-163c93f1988d';
var path = {
    nugetPath: 'D:\\Program Files\\nuget\\nuget.exe',
    nugetOutPath: 'D:\\Repositories\\Nuget',
    nugetSource: 'https://www.nuget.org/api/v2/package'
}

gulp.task('nuget-pack', function () {
    return gulp.src(projectName + '.csproj')
        .pipe(nuget.pack({
            nuget: path.nugetPath,
            outputDirectory: path.nugetOutPath, //'./nupkgs/',
            //version: '1.0.0',
            //basePath: './',
            exclude: 'gulpfile.js',//'**/*.designer.cs',
            //properties: 'configuration=release',
            //minClientVersion: '2.5',
            //msBuildVersion: '12',
            //build: true,
            //symbols: true,
            //excludeEmptyDirectories: true,
            includeReferencedProjects: true,
            //noDefaultExcludes: true,
            //tool: true
        }));
});

gulp.task('nuget-push', function() {
    return gulp.src(projectName + '.csproj')
        .pipe(nuget.pack({
            nuget: path.nugetPath,
            outputDirectory: path.nugetOutPath,
            exclude: 'gulpfile.js',
            properties: 'configuration=release',
            includeReferencedProjects: true,
        }))
     .pipe(nuget.push({ source: path.nugetSource, nuget: path.nugetPath, apiKey: apiKey }));
})