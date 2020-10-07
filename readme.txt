フォルダ構成(暫定)
・Build:	makeファイルやvisual studioのプロジェクトファイルなど、ビルドに必要なもの
・Source:	ソースコード (.cppファイルや、ユーザから参照されないヘッダファイル)
・Include:	ユーザから参照されるヘッダファイル
・Library:	ビルドによってできた、ユーザから参照されるlibファイル
・Tool:		ビルドによってできた、ユーザから利用されるツール (例:レイヤコンパイラ)
・ExtLib:	ビルドに必要な外部ライブラリ
・Object:	ビルド時にできる中間ファイル
・Samples:	サンプルプログラム
・Document:	ドキュメント類
・Test:		テストプログラムと、テストするための場所の提供
			ビルド時のアセンブリは、ここに出力される
			テストを終えたら、開発者がLibraryやToolにアセンブリを移す

# ROS package

1. Clone this project into your catkin workspace
2. catkin build

Refer following URL how to use RTCOP from your ROS project
https://github.com/hisazumi/crostest
