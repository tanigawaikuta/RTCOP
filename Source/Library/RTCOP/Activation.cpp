//================================================================================
// Activation.cpp
//
// 役割: ユーザにレイヤアクティベート、ディアクティベートの機能を提供する。
//       レイヤの指定は、レイヤごとのIDによって行う。
//================================================================================

#include "RTCOP/Activation.h"
#include "RTCOP/Framework.h"
#include "RTCOP/Core/LayerActivater.h"

namespace RTCOP {

// アクティベート
void Activate(int layerid)
{
	Framework::Instance->GetLayerActivater()->ActivateLayer(layerid);
}

// ディアクティベート
void Deactivate(int layerid)
{
	Framework::Instance->GetLayerActivater()->DeactivateLayer(layerid);
}

} // namespace RTCOP {}
