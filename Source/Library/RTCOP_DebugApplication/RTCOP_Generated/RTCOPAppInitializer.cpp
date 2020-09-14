//================================================================================
// RTCOPAppInitializer.cpp
//
// 役割: RTCOPの初期化を行うクラス。
//       このクラスはレイヤ記述を基に、自動生成される。
//================================================================================

#include "RTCOP_Generated/RTCOPAppInitializer.h"
#include "RTCOP/Core/RTCOPManager.h"
#include "RTCOP/Core/LayerdObjectInitializer.h"
#include "RTCOP/Core/LayerActivater.h"
// 登録するレイヤ
#include "RTCOP_Generated/BaseLayer.h"
#include "RTCOP_Generated/JapaneseLayer.h"
#include "RTCOP_Generated/EnglishLayer.h"

namespace RTCOP {
namespace Generated {

// コンストラクタ
RTCOPAppInitializer::RTCOPAppInitializer()
{
}

// デストラクタ
RTCOPAppInitializer::~RTCOPAppInitializer()
{
}

// オブジェクトの生成
Core::RTCOPManager* RTCOPAppInitializer::InitializeRTCOPManager()
{
	// レイヤ、クラス、メソッドの数を設定
	int numOfLayers = 3;
	int numOfClasses = 1;
	int* numOfMethods = new int[numOfClasses]{ 4 };
#if defined(LINUX_X86) || defined(LINUX_X64) || defined(LINUX_ARM) || defined(LINUX_ARM64) || defined(WIN32_MINGW) || defined(WIN64_MINGW) || defined(MACOS_X64)
	for (int i = 0; i < numOfClasses; ++i)
		++numOfMethods[i];
#endif
	// マネージャクラスの生成と返却
	return new Core::RTCOPManager(numOfLayers, numOfClasses, numOfMethods);
}

// オブジェクトの生成
Core::LayerdObjectInitializer* RTCOPAppInitializer::InitializeLayerdObjectInitializer(Core::RTCOPManager* manager)
{
	return new Core::LayerdObjectInitializer(manager);
}

// オブジェクトの生成
Core::LayerActivater* RTCOPAppInitializer::InitializeLayerActivater(Core::RTCOPManager* manager)
{
	return new Core::LayerActivater(manager);
}

// レイヤの登録
void RTCOPAppInitializer::RegisterLayers(Core::RTCOPManager* manager)
{
	int numOfBaseClasses = manager->GetNumOfBaseClasses();
	int* numOfBaseMethods = manager->GetNumOfBaseMethods();
	// レイヤ登録
	manager->RegisterLayer(new BaseLayer(0, "BaseLayer", numOfBaseClasses, numOfBaseMethods));
	manager->RegisterLayer(new JapaneseLayer(1, "JapaneseLayer", numOfBaseClasses, numOfBaseMethods));
	manager->RegisterLayer(new EnglishLayer(2, "EnglishLayer", numOfBaseClasses, numOfBaseMethods));
}

// 初回レイヤアクティベーション
void RTCOPAppInitializer::FirstLayerActivation(Core::LayerActivater* activater)
{
	// ベースレイヤのアクティベート
	activater->ActivateLayer(0);
}

//-------------------------------------------------
// デフォルトイニシャライザの取得
//-------------------------------------------------
Core::Initializer* _GetDefaultInitializer_()
{
	return new RTCOPAppInitializer();
}

} // namespace Generated {}
} // namespace RTCOP {}
