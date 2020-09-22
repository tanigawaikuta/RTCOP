#include "Generated/API.h"
#include "Generated/BaseLayer.h"
#include <stdio.h>

using namespace RTCOP;

int main()
{
	// RTCOPの初期化
	RTCOP::Framework::Instance->Initialize();

	// Aのインスタンス化
	baselayer::SuperA* a = copnew<baselayer::SubA>();
	// レイヤアクティベート
	activate(Generated::LayerID::Layer1);
	// メソッド実行
	a->m1();

	// RTCOPの終了処理
	RTCOP::Framework::Instance->Finalize();

	return 0;
}
