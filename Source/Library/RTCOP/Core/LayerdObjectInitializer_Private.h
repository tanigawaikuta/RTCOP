//================================================================================
// LayerdObjectInitializer_Private.h
//
// 役割: LayerdObjectInitializerのプライベートクラス。
//================================================================================

#ifndef __RTCOP_CORE_LAYERDOBJECTINITIALIZER_PRIVATE__
#define __RTCOP_CORE_LAYERDOBJECTINITIALIZER_PRIVATE__

namespace RTCOP {
namespace Core {

class RTCOPManager;

// 与えられたレイヤードなオブジェクトの初期化を行う
class LayerdObjectInitializer_Private
{
public:
	// コンストラクタ
	LayerdObjectInitializer_Private(RTCOPManager* manager);
	// デストラクタ
	~LayerdObjectInitializer_Private();
public:
	// RTCOPの管理者
	RTCOPManager* _Manager;
};

} // namespace Core {}
} // namespace RTCOP {}

#endif // !__RTCOP_CORE_LAYERDOBJECTINITIALIZER_PRIVATE__
