//================================================================================
// LayerActivater.h
//
// 役割: レイヤのアクティベーション、ディアクティベーションを行うクラス。
//================================================================================

#ifndef __RTCOP_CORE_LAYERACTIVATER__
#define __RTCOP_CORE_LAYERACTIVATER__

namespace RTCOP {
namespace Core {

class RTCOPManager;
class Layer;

// レイヤのアクティベーション、ディアクティベーションを行う
class LayerActivater
{
public:
	// コンストラクタ
	explicit LayerActivater(RTCOPManager* manager);
	// デストラクタ
	virtual ~LayerActivater();
public:
	// レイヤアクティベート
	virtual void ActivateLayer(int layerid);
	// レイヤディアクティベート
	virtual void DeactivateLayer(int layerid);
protected:
	// プライベートクラス
	class LayerActivater_Private* _Private;
};

} // namespace Core {}
} // namespace RTCOP {}

#endif // !__RTCOP_CORE_LAYERACTIVATER__
