#include "Generated/API.h"
#include "Generated/BaseLayer.h"
#include <stdio.h>

using namespace RTCOP;

int main()
{
	// RTCOPの初期化
	RTCOP::Framework::Instance->Initialize();

	// Aのインスタンス化
	baselayer::A* a = copnew<baselayer::A>();
	// レイヤアクティベート
	activate(Generated::LayerID::Layer1);
	activate(Generated::LayerID::Layer2);
	// メソッド実行
	a->m1();

	// RTCOPの終了処理
	RTCOP::Framework::Instance->Finalize();

	return 0;
}
