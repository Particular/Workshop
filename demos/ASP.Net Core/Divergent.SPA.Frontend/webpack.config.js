"use strict";
var webpack = require("webpack");
var CopyWebpackPlugin = require('copy-webpack-plugin');
//var TSLintPlugin = require('tslint-webpack-plugin');

module.exports = {
    entry: {app: "./app/main.ts"},
    //debug: true,
    output: {
        path: __dirname + "/wwwroot",
        filename: "dist/scripts/[name].bundle.js",
        sourceMapFilename: "dist/scripts/[name].bundle.js.map"
}, resolve: {
        extensions: [".webpack.js", ".web.js", ".ts", ".js"]
    },
   
    // Turn on sourcemaps
    devtool: "source-map",
    module: {
        //preLoaders: [
        //    {
        //        test: /\.ts$/,
        //        exclude: /(node_modules)[\\\/]/,
        //        loader: "tslint-loader"
        //    }
        //],
        loaders: [
            {
                test: /\.tsx?$/,
                exclude: /(node_modules)[\\\/]/,
                loaders: ["ts-loader"]
            }
        ]
    },
    //tslint: {
    //    configFile: "./tslint.json",
    //    emitErrors: true,
    //    failOnHint: true
    //},
    // Add minification
    plugins: [
        new webpack.optimize.UglifyJsPlugin({
            minimize: true,
            sourceMap: true,
            beautify: false,
            output: {
                comments: false
            },
            compressor: {
                warnings: false,
                drop_console: true
            },
            mangle: {
                except: ["$", "webpackJsonp"],
                screw_ie8: true, // Don"t care about IE8  
                //keep_fnames: true // Don"t mangle function names
            }
        }),
        new CopyWebpackPlugin([
            { from: __dirname + '/node_modules/jquery/dist/jquery.min.js', to: 'lib/scripts/jquery.min.js' },
            { from: __dirname + '/node_modules/bootstrap/dist/js/bootstrap.min.js', to: 'lib/scripts/bootstrap.min.js' },
            { from: __dirname + '/node_modules/bootstrap/dist/fonts', to: 'lib/fonts' },
            { from: __dirname + '/node_modules/bootstrap/dist/css/bootstrap.min.css', to: 'lib/styles/bootstrap.min.css' },
            { from: __dirname + '/styles', to: 'dist/styles' }
        ])//,
        //new TSLintPlugin({
        //    files: [__dirname + '/app/**/*.ts']
        //})
    ]
};