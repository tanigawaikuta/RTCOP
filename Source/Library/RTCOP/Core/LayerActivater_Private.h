//================================================================================
// LayerActivater_Private.h
//
// 役割: LayerActivaterのプライベートクラス。
//================================================================================

#ifndef __RTCOP_CORE_LAYERACTIVATER_PRIVATE__
#define __RTCOP_CORE_LAYERACTIVATER_PRIVATE__

namespace RTCOP {
namespace Core {

class RTCOPManager;
class Layer;

// レイヤのアクティベーション、ディアクティベーションを行う
class LayerActivater_Private
{
public:
	// コンストラクタ
	LayerActivater_Private(RTCOPManager* manager);
	// デストラクタ
	virtual ~LayerActivater_Private();
public:
	// RTCOPの管理者
	RTCOPManager* _Manager;
	// 現在のトップレイヤ
	Layer* _CurrentTopLayer;
};

} // namespace Core {}
} // namespace RTCOP {}

#endif // !__RTCOP_CORE_LAYERACTIVATER_PRIVATE__
