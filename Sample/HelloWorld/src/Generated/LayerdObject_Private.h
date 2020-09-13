//================================================================================
// LayerdObject_Private.h
//
// 役割: LayerdObjectのプライベートクラス。
//================================================================================

#ifndef __RTCOP_CORE_LAYERDOBJECT_PRIVATE__
#define __RTCOP_CORE_LAYERDOBJECT_PRIVATE__

namespace RTCOP {
namespace Core {

class PartialClassMembers;

// レイヤードなオブジェクトを表すテンプレートクラス
class LayerdObject_Private
{
public:
	// コンストラクタ
	LayerdObject_Private();
	// デストラクタ
	~LayerdObject_Private();
public:
	// レイヤードオブジェクトの共通の要素を初期化
	void InitializeLayerdObject(void* obj, int numOfLayers);
public:
	// LayerdObjectのポインタ
	void* _LayerdObjectPtr;
	// 各パーシャルクラスのメンバ変数群
	PartialClassMembers** _PartialClassMembers;
	// レイヤの個数
	int _NumOfLayers;
};

} // namespace Core {}
} // namespace RTCOP {}

#endif // !__RTCOP_CORE_LAYERDOBJECT_PRIVATE__
