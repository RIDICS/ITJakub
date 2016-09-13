/// <binding BeforeBuild='build' Clean='clean' />
/*
This file in the main entry point for defining Gulp tasks and using Gulp plugins.
Click here to learn more. http://go.microsoft.com/fwlink/?LinkId=518007
*/

var gulp = require("gulp"),
  rimraf = require("rimraf"),
  concat = require("gulp-concat"),
  cssmin = require("gulp-cssmin"),
  less = require("gulp-less"),
  uglify = require("gulp-uglify");

var paths = {
    webroot: "./wwwroot/"
};

paths.less = paths.webroot + "css/**/*.less";
paths.css = paths.webroot + "css/**/*.css";
paths.areaLess = paths.webroot + "Areas/*/css/**/*.less";
paths.areaCss = paths.webroot + "Areas/*/css/**/*.css";


// Clean-up

gulp.task("clean:css", function (cb) {
    rimraf(paths.css, cb);
});

gulp.task("clean:css_areas", function (cb) {
    rimraf(paths.areaCss, cb);
});

gulp.task("clean", ["clean:css", "clean:css_areas"]);


// Less build

gulp.task("build:less_root", function () {
    return gulp.src(paths.less)
        .pipe(less({
            relativeUrls: true
        }))
        .pipe(gulp.dest(paths.webroot + "css"));
});

gulp.task("build:less_areas", function () {
    return gulp.src(paths.areaLess)
        .pipe(less({
            relativeUrls: true
        }))
        .pipe(gulp.dest(paths.webroot + "Areas"));
});

gulp.task("build:less", ["build:less_root", "build:less_areas"]);


// Main build

gulp.task("build", ["build:less"]);