/// <binding AfterBuild='run:webpack' Clean='clean' />
/*
This file in the main entry point for defining grunt tasks and using grunt plugins.
Click here to learn more. http://go.microsoft.com/fwlink/?LinkID=513275&clcid=0x409
*/
module.exports = function (grunt) {
    grunt.initConfig({
        run: {
            webpack: {
                cmd: "node.exe",
                args: ['node_modules\\webpack\\bin\\webpack.js', '--colors']
            }, 'webpack-watch': {
                cmd: "node.exe",
                args: ['node_modules\\webpack\\bin\\webpack.js', '--colors', '--watch']
            }
        },
        clean: {
            build: {
                src: ['Scripts/dist']
            }
        }
    });

    grunt.loadNpmTasks('grunt-run');
    grunt.loadNpmTasks('grunt-contrib-clean');

    grunt.registerTask('dev', [
        'clean:build',
        'run:webpack'
    ]);
    grunt.registerTask('watch', [
        'clean:build',
        'run:webpack-watch'
    ]);
};