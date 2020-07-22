#include "RTCOP_Generated/API.h"
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
	hello->Print2('a');
	// JapaneseLayerをアクティベート
	activate(RTCOP::Generated::LayerID::JapaneseLayer);
	hello->Print();
	hello->Print2('b');
	// EnglishLayerをアクティベート
	activate(RTCOP::Generated::LayerID::EnglishLayer);
	hello->Print();
	hello->Print2('c');
	// JapaneseLayerをディアクティベート
	deactivate(RTCOP::Generated::LayerID::JapaneseLayer);
	hello->Print();
	hello->Print2('d');
	// Helloのdelete
	// 元のHelloでデストラクタが定義されていない場合、キャストしないとデストラクタが実行されない…
	delete ((Core::LayerdObject<Hello>*)hello);

	// RTCOPの終了処理
	RTCOP::Framework::Instance->Finalize();

	return 0;
}
