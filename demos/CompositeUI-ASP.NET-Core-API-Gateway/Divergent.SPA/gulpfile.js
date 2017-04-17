/// <binding AfterBuild='default' Clean='clean' />
/*
This file is the main entry point for defining Gulp tasks and using Gulp plugins.
Click here to learn more. http://go.microsoft.com/fwlink/?LinkId=518007
*/

var gulp = require('gulp');
var del = require('del');

var paths = {
    app: ['app/**/*.js', 'app/**/*.ts', 'app/**/*.map'],
    libs: ['node_modules/jquery/dist/jquery.min.js',
        'node_modules/bootstrap/dist/js/bootstrap.min.js'],
    styles: ['node_modules/bootstrap/dist/css/bootstrap.min.css',
        'node_modules/bootstrap/dist/css/bootstrap-theme.min.css'],
    fonts: ['node_modules/bootstrap/dist/fonts/*.*'],
};

gulp.task('clean', function () {
    return del(['wwwroot/scripts/**/*',
        'wwwroot/style/**/*',
        'wwwroot/fonts/**/*']);
});

gulp.task('lib', function () {
    gulp.src(paths.libs).pipe(gulp.dest('wwwroot/scripts/lib'))
});

gulp.task('style', function () {
    gulp.src(paths.styles).pipe(gulp.dest('wwwroot/style'))
});

gulp.task('fonts', function () {
    gulp.src(paths.fonts).pipe(gulp.dest('wwwroot/fonts'))
});

gulp.task('default', ['lib', 'style', 'fonts'], function () {
    gulp.src(paths.app).pipe(gulp.dest('wwwroot/scripts'))
});