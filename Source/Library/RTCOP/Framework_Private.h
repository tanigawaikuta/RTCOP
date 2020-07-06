//================================================================================
// Framework_Private.h
//
// 役割: Frameworkのプライベートクラス
//================================================================================

#ifndef __RTCOP_FRAMEWORK_PRIVATE__
#define __RTCOP_FRAMEWORK_PRIVATE__

namespace RTCOP {

// 機能提供を行うオブジェクトの宣言
namespace Core {
class RTCOPManager;
class LayerdObjectInitializer;
class LayerActivater;
} // namespace Core {}

// RTCOPのフレームワーク
class Framework_Private
{
public:
	// コンストラクタ
	Framework_Private();
	// デストラクタ
	~Framework_Private();
	// RTCOPマネージャ
	Core::RTCOPManager* _RTCOPManager;
	// LayerdObject初期化
	Core::LayerdObjectInitializer* _LayerdObjectInitializer;
	// レイヤアクティベータ
	Core::LayerActivater* _LayerActivater;
};

} // namespace RTCOP {}

#endif // !__RTCOP_FRAMEWORK_PRIVATE__
