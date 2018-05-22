"use strict";
var webpack = require("webpack");
module.exports = {
    entry: {app: "./app/main.ts"},
	debug: true,
	output: {
		path:"./wwwroot/dist",
		filename: "[name].bundle.js",
		sourceMapFilename: "[name].bundle.js.map"
}, resolve: {
        extensions: ["", ".webpack.js", ".web.js", ".ts", ".js"]
    },
   
    // Turn on sourcemaps
    devtool: "source-map",
    module: {
        preLoaders: [
            {
                test: /\.ts$/,
                exclude: /(node_modules)[\\\/]/,
                loader: "tslint-loader"
            }
        ],
        loaders: [
            {
                test: /\.tsx?$/,
                exclude: /(node_modules)[\\\/]/,
                loaders: ["ts-loader"]
            }
        ]
    },
    tslint: {
        configFile: "./tslint.json",
        emitErrors: true,
        failOnHint: true
    },
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
        })
    ]
};