const path = require('path');
const HtmlWebpackPlugin = require('html-webpack-plugin');
const { CleanWebpackPlugin } = require('clean-webpack-plugin');
const EslintPlugin = require('eslint-webpack-plugin');

module.exports = (env, options) => {
  const isProduction = options.mode === 'production';
  const config = {
    entry: './src/index.tsx',
    plugins: [
      new HtmlWebpackPlugin({
        title: 'LxHt3Team24 - Web Bot',
        template: './src/index.html',
        favicon: './src/favicon.ico',
      }),
      new CleanWebpackPlugin(),
      new EslintPlugin({ extensions: 'tsx' }),
    ],
    devtool: isProduction ? false : 'inline-source-map',
    resolve: {
      extensions: ['.tsx', '.ts', '.js'],
    },
    devServer: {
      static: './dist',
      historyApiFallback: true,
    },
    output: {
      filename: '[name].js',
      path: path.resolve(__dirname, 'dist'),
      clean: true,
    },
    optimization: {
      runtimeChunk: 'single',
    },
    mode: isProduction ? 'production' : 'development',
    // watch: !isProduction,
    module: {
      rules: [
        {
          test: /\.tsx?$/,
          include: path.resolve(__dirname, 'src'),
          use: 'ts-loader'
        },
        {
          test: /\.s?[ac]ss$/i,
          use: ['style-loader', 'css-loader', 'sass-loader'],
        },
        {
          test: /\.(?:ico|png|svg|jpg|jpeg|gif)$/i,
          type: 'asset/resource',
        },
        {
          test: /\.mp3$/i,
          type: 'asset/resource',
        },
        {
          test: /\.json$/i,
          loader: 'json5-loader',
          type: 'javascript/auto',
        },
      ],
    },
  };
  return config;
};
