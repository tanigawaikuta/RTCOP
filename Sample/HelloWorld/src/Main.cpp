#include "Generated/API.h"
#include "Generated/BaseLayer.h"

using namespace RTCOP;

int main()
{
	// RTCOPの初期化
	RTCOP::Framework::Instance->Initialize();

	// Helloのインスタンス化
	baselayer::Hello* hello = copnew<baselayer::Hello>();

	// ベースメソッドの実行
	hello->Print();

	// JapaneseLayerをアクティベート
	activate(RTCOP::Generated::LayerID::Japanese);
	hello->Print();
	
	// EnglishLayerをアクティベート
	deactivate(RTCOP::Generated::LayerID::Japanese);
	activate(RTCOP::Generated::LayerID::English);
	hello->Print();

	// Helloのdelete
	delete hello;

	// RTCOPの終了処理
	RTCOP::Framework::Instance->Finalize();

	return 0;
}
