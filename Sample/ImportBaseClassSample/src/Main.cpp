#include "Generated/API.h"
#include "Hello.h"

using namespace RTCOP;

int main()
{
	// RTCOPの初期化
	RTCOP::Framework::Instance->Initialize();

	// Helloのインスタンス化
	Hello* hello = copnew<Hello>();

	// ベースメソッドの実行
	hello->Print();

	// JapaneseLayerをアクティベート
	activate(RTCOP::Generated::LayerID::Japanese);
	hello->Print();

	// Helloのdelete
	delete hello;

	// RTCOPの終了処理
	RTCOP::Framework::Instance->Finalize();

	return 0;
}
