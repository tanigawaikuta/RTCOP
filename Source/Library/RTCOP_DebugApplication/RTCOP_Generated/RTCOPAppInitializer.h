//================================================================================
// RTCOPAppInitializer.h
//
// 役割: RTCOPの初期化を行うクラス。
//       このクラスはレイヤ記述を基に、自動生成される。
//================================================================================

#ifndef __RTCOP_GENERATED_RTCOPAPPINITIALIZER__
#define __RTCOP_GENERATED_RTCOPAPPINITIALIZER__

// このクラスを継承する
#include "RTCOP/Core/Initializer.h"

namespace RTCOP {

namespace Core {
// 初期化するオブジェクトの宣言
class RTCOPManager;
class LayerdObjectInitializer;
class LayerActivater;

}

namespace Generated {

// RTCOPの初期化を行うクラス
class RTCOPAppInitializer : public Core::Initializer
{
public:
	// コンストラクタ・デストラクタ
	explicit RTCOPAppInitializer();
	virtual ~RTCOPAppInitializer();
	// オブジェクトの生成
	virtual Core::RTCOPManager* InitializeRTCOPManager() override;
	virtual Core::LayerdObjectInitializer* InitializeLayerdObjectInitializer(Core::RTCOPManager* manager) override;
	virtual Core::LayerActivater* InitializeLayerActivater(Core::RTCOPManager* manager) override;
	// レイヤの登録
	virtual void RegisterLayers(Core::RTCOPManager* manager) override;
	// 初回レイヤアクティベーション
	virtual void FirstLayerActivation(Core::LayerActivater* activater) override;
};

} // namespace Generated {}
} // namespace RTCOP {}

#endif // !__RTCOP_GENERATED_RTCOPAPPINITIALIZER__
