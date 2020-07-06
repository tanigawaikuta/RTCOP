//================================================================================
// BaseLayer.cpp
//
// 役割: BaseLayerを表すクラス。自動生成。
//================================================================================

#include "RTCOP_Generated/API.h"
#include "RTCOP_Generated/BaseLayer.h"
#include "RTCOP_Generated/Layer_Private.h"
#include "RTCOP_Generated/LayerdObject_Private.h"
#include "RTCOP/Framework.h"
#include "RTCOP/Core/RTCOPManager.h"
#include "RTCOP/Core/PartialClassMembers.h"
#include "RTCOP_Generated/DependentCode/GetVirtualFunctionTable.h"
#include <cstring>

namespace RTCOP {
namespace Generated {

// インスタンスの取得
BaseLayer* BaseLayer::GetInstance()
{
	return (BaseLayer*)RTCOP::Framework::Instance->GetRTCOPManager()->GetLayer((int)LayerID::BaseLayer);
}

// コンストラクタ
BaseLayer::BaseLayer(const int id, const char* const name, int numOfBaseClasses, int* numOfBaseMethods)
	:Core::Layer(id, name, numOfBaseClasses, numOfBaseMethods)
{
	// Helloクラスの仮想関数テーブル
	int size0 = sizeof(volatile void*) * numOfBaseMethods[0];
	volatile void** virtualFunctionTable0 = DependentCode::HelloClass::GetVirtualFunctionTable(this);
	std::memcpy(_Private->_VirtualFunctionTables[0], virtualFunctionTable0, size0);
}

// デストラクタ
BaseLayer::~BaseLayer()
{
}

// レイヤードオブジェクトの初期化
void* BaseLayer::InitializeLayerdObject(void* obj, int classID)
{
	return obj;
}

} // namespace Generated {}
} // namespace RTCOP {}
