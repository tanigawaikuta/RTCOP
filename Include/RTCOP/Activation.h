//================================================================================
// Activation.h
//
// 役割: ユーザにレイヤアクティベート、ディアクティベートの機能を提供する。
//       レイヤの指定は、レイヤごとのIDによって行う。
//================================================================================

#ifndef __RTCOP_ACTIVATION__
#define __RTCOP_ACTIVATION__

namespace RTCOP {

// アクティベート
void Activate(int layerid);
// ディアクティベート
void Deactivate(int layerid);

} // namespace RTCOP {}

#endif // !__RTCOP_ACTIVATION__
