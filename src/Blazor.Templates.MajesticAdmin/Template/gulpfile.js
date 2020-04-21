'use strict'

var gulp = require('gulp');
var browserSync = require('browser-sync').create();
var sass = require('gulp-sass');
var rename = require('gulp-rename');
var del = require('del');
var replace = require('gulp-replace');
var injectPartials = require('gulp-inject-partials');
var inject = require('gulp-inject');
var sourcemaps = require('gulp-sourcemaps');
var concat = require('gulp-concat');
var merge = require('merge-stream');
var babel = require('gulp-babel');

gulp.paths = {
    dist: 'dist',
};

var paths = gulp.paths;

gulp.task('sass', function () {
    return gulp.src('./scss/**/style.scss')
        .pipe(sourcemaps.init())
        .pipe(sass({outputStyle: 'expanded'}).on('error', sass.logError))
        .pipe(sourcemaps.write('../wwwroot/maps'))
        .pipe(gulp.dest('../wwwroot/css'))
        .pipe(browserSync.stream());
});

/*replace image path and linking after injection*/
gulp.task('replacePath', function(){
    var replacePath1 = gulp.src(['./pages/*/*.html'], { base: "./" })
        .pipe(replace('="images/', '="../../images/'))
        .pipe(replace('href="pages/', 'href="../../pages/'))
        .pipe(replace('href="documentation/', 'href="../../documentation/'))
        .pipe(replace('href="index.html"', 'href="../../index.html"'))
        .pipe(gulp.dest('.'));
    var replacePath2 = gulp.src(['./pages/*.html'], { base: "./" })
        .pipe(replace('="images/', '="../images/'))
        .pipe(replace('"pages/', '"../pages/'))
        .pipe(replace('href="index.html"', 'href="../index.html"'))
        .pipe(gulp.dest('.'));
    var replacePath3 = gulp.src(['./index.html'], { base: "./" })
        .pipe(replace('="images/', '="images/'))
        .pipe(gulp.dest('.'));
    return merge(replacePath1, replacePath2, replacePath3);
});

gulp.task('clean:vendors', function () {
    return del([
      '../wwwroot/vendors/**/*'
    ], { "force": true });
});

/*Building vendor scripts needed for basic template rendering*/
gulp.task('buildBaseVendorScripts', function() {
    return gulp.src([
			'./node_modules/jquery/dist/jquery.min.js', 
			'./node_modules/popper.js/dist/umd/popper.min.js', 
			'./node_modules/bootstrap/dist/js/bootstrap.min.js', 
			'./node_modules/perfect-scrollbar/dist/perfect-scrollbar.min.js'
		])
	  /*.pipe(babel({
		  "presets": [
			[
			  "@babel/preset-env",
			  {
				"targets": {
					"chrome": "58",
					"ie": "11"
				  }
			  }
			]
		  ]
		}))*/
      .pipe(concat('vendor.bundle.base.js'))
      .pipe(gulp.dest('../wwwroot/vendors/base'));
});

/*Building vendor styles needed for basic template rendering*/
gulp.task('buildBaseVendorStyles', function() {
    return gulp.src(['./node_modules/perfect-scrollbar/css/perfect-scrollbar.css'])
      .pipe(concat('vendor.bundle.base.css'))
      .pipe(gulp.dest('../wwwroot/vendors/base'));
});

gulp.task('copyRecursiveVendorFiles', function() {
    var vFile1 = gulp.src(['./node_modules/chart.js/dist/Chart.min.js'])
        .pipe(gulp.dest('../wwwroot/vendors/chart.js'));
    var vFile2 = gulp.src(['./node_modules/datatables.net/js/jquery.dataTables.js'])
        .pipe(gulp.dest('../wwwroot/vendors/datatables.net'));
    var vFile3 = gulp.src(['./node_modules/datatables.net-bs4/js/dataTables.bootstrap4.js'])
        .pipe(gulp.dest('../wwwroot/vendors/datatables.net-bs4'));
    var vFile4 = gulp.src(['./node_modules/datatables.net-bs4/css/dataTables.bootstrap4.css'])
        .pipe(gulp.dest('../wwwroot/vendors/datatables.net-bs4'));
    var vFile5 = gulp.src(['./node_modules/@mdi/font/css/materialdesignicons.min.css'])
        .pipe(gulp.dest('../wwwroot/vendors/mdi/css'));
    var vFile6 = gulp.src(['./node_modules/@mdi/font/fonts/*'])
        .pipe(gulp.dest('../wwwroot/vendors/mdi/fonts'));
    return merge(vFile1, vFile2, vFile3, vFile4, vFile5, vFile6);
});

//Copy essential map files
gulp.task('copyMapFiles', function() {
    var map1 = gulp.src('node_modules/bootstrap/dist/js/bootstrap.min.js.map')
        .pipe(gulp.dest('../wwwroot/vendors/base'));
    var map2 = gulp.src('node_modules/@mdi/font/css/materialdesignicons.min.css.map')
        .pipe(gulp.dest('../wwwroot/vendors/mdi/css'));
    return merge(map1, map2);
});

/*sequence for building vendor scripts and styles*/
gulp.task('bundleVendors', gulp.series('clean:vendors','buildBaseVendorStyles','buildBaseVendorScripts','copyRecursiveVendorFiles', 'copyMapFiles'));

gulp.task('default', gulp.series('bundleVendors'));
