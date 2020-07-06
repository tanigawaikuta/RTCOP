//================================================================================
// ActivationForApp.h
//
// 役割: 固有のアプリケーションに向けて、
// アクティベーションするレイヤをenumで指定できるようにする。
//================================================================================

#ifndef __RTCOP_GENERATED_ACTIVATIONFORAPP__
#define __RTCOP_GENERATED_ACTIVATIONFORAPP__

// 元々のactivationを読み込む
#include "RTCOP/Activation.h"

namespace RTCOP {
namespace Generated {
// レイヤID
enum class LayerID : int
{
	BaseLayer = 0,
	JapaneseLayer = 1,
	EnglishLayer = 2,
};
} // namespace Generated {}

// アクティベート
inline void activate(Generated::LayerID layerid)
{
	Activate((int)layerid);
}
// ディアクティベート
inline void deactivate(Generated::LayerID layerid)
{
	Deactivate((int)layerid);
}

} // namespace RTCOP {}

#endif // !__RTCOP_GENERATED_ACTIVATION__
