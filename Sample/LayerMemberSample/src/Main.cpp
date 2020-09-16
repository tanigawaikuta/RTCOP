#include "Generated/API.h"
#include "Generated/BaseLayer.h"

using namespace RTCOP;

int main()
{
	// RTCOPの初期化
	RTCOP::Framework::Instance->Initialize();

	// Sampleのインスタンス化
	baselayer::Sample* sample = copnew<baselayer::Sample>(1990);

	// ベースメソッドの実行
	sample->Print();

	// Layer1をアクティベート
	activate(RTCOP::Generated::LayerID::Layer1);
	sample->Print();

	// Sampleのdelete
	delete sample;

	// RTCOPの終了処理
	RTCOP::Framework::Instance->Finalize();

	return 0;
}
