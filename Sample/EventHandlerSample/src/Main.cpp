#include "Generated/API.h"
#include "Generated/BaseLayer.h"
#include <stdio.h>

using namespace RTCOP;

int main()
{
	// RTCOPの初期化
	RTCOP::Framework::Instance->Initialize();

	// Sampleのインスタンス化
	printf("baselayer::Sampleをインスタンス化する\n");
	baselayer::Sample* sample = copnew<baselayer::Sample>();
	printf("\n");

	// Layer1をアクティベート
	printf("Layer1をアクティベートする\n");
	activate(RTCOP::Generated::LayerID::Layer1);
	printf("\n");

	// Layer1をディアクティベート
	printf("Layer1をディアクティベートする\n");
	deactivate(RTCOP::Generated::LayerID::Layer1);
	printf("\n");

	// Sampleのdelete
	printf("baselayer::Sampleをdeleteする\n");
	delete sample;

	// RTCOPの終了処理
	RTCOP::Framework::Instance->Finalize();

	return 0;
}
