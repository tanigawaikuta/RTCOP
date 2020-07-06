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
	LayerdObject_Private();
	~LayerdObject_Private();
public:
	// レイヤードオブジェクトの共通の要素を初期化
	void InitializeLayerdObject(void* obj, int baseClassSize, int numOfLayers);
public:
	// LayerdObjectのポインタ
	void* _LayerdObjectPtr;
	// 各パーシャルクラスのメンバ変数群
	PartialClassMembers** _PartialClassMembers;
	// ベースクラスのサイズ
	int _BaseClassSize;
	// レイヤの個数
	int _NumOfLayers;
};

} // namespace Core {}
} // namespace RTCOP {}

#endif // !__RTCOP_CORE_LAYERDOBJECT_PRIVATE__
