//================================================================================
// Framework.h
//
// 役割: ユーザにRTCOPのフレームワークを提供する。RTCOPアプリケーションでは、
//       開始・終了時に、このヘッダで宣言されたメソッドを必ず実行しなければならない。
//================================================================================

#ifndef __RTCOP_FRAMEWORK__
#define __RTCOP_FRAMEWORK__

namespace RTCOP {

// 機能提供を行うオブジェクトの宣言
namespace Core {
class Initializer;
class RTCOPManager;
class LayerdObjectInitializer;
class LayerActivater;
} // namespace Core {}

// RTCOPのフレームワーク
class Framework
{
public:
	// シングルトン
	static Framework* const Instance;
public:
	// RTCOPの初期化 最初に実行すること
	virtual void Initialize();
	// RTCOPの初期化 最初に実行すること (初期化方法をカスタマイズ可能)
	virtual void Initialize(Core::Initializer* initializer);
	// RTCOPの終了処理 最後に実行すること
	virtual void Finalize();
	// RTCOPマネージャの取得
	Core::RTCOPManager* const GetRTCOPManager();
	// LayerdObject初期化の取得
	Core::LayerdObjectInitializer* const GetLayerdObjectInitializer();
	// レイヤアクティベータの取得
	Core::LayerActivater* const GetLayerActivater();
protected:
	// コンストラクタ
	Framework();
	// デストラクタ
	virtual ~Framework();
protected:
	// プライベートクラス
	class Framework_Private* _Private;
};

} // namespace RTCOP {}

#endif // !__RTCOP_FRAMEWORK__
