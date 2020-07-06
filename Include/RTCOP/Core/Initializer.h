//================================================================================
// Initializer.h
//
// 役割: RTCOPの初期化を行うクラス。
//       RTCOPの自動生成コードでこのクラスを継承したクラスを定義し、
//       それを基に、RTCOPの初期化を実行する。
//================================================================================

#ifndef __RTCOP_CORE_INITIALIZER__
#define __RTCOP_CORE_INITIALIZER__

namespace RTCOP {
namespace Core {

// 初期化するオブジェクトの宣言
class RTCOPManager;
class LayerdObjectInitializer;
class LayerActivater;

// RTCOPの初期化を行うクラス
class Initializer
{
public:
	// コンストラクタ
	Initializer();
	// デストラクタ
	virtual ~Initializer();
public:
	// RTCOPManagerの生成
	virtual RTCOPManager* InitializeRTCOPManager() = 0;
	// LayerdObjectInitializerの生成
	virtual LayerdObjectInitializer* InitializeLayerdObjectInitializer(RTCOPManager* manager) = 0;
	// LayerActivaterの生成
	virtual LayerActivater* InitializeLayerActivater(RTCOPManager* manager) = 0;
	// レイヤの登録
	virtual void RegisterLayers(RTCOPManager* manager) = 0;
	// 初回レイヤアクティベーション
	virtual void FirstLayerActivation(LayerActivater* activater) = 0;
};

} // namespace Core {}
} // namespace RTCOP {}

#endif // !__RTCOP_CORE_INITIALIZER__
