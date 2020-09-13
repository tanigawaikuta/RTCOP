//================================================================================
// BaseLayer.h
//
// 役割: BaseLayerを表すクラス。自動生成。
//================================================================================

#ifndef __RTCOP_GENERATED_BASELAYER__
#define __RTCOP_GENERATED_BASELAYER__

// 機能実現のためのヘッダ
#include "RTCOP/Core/Layer.h"
#include "RTCOP/Core/LayerdObject.h"

namespace RTCOP {
namespace Generated {

class BaseLayer : public Core::Layer
{
public:
	// インスタンスの取得
	static BaseLayer* GetInstance();
public:
	// コンストラクタ・デストラクタ
	explicit BaseLayer(const int id, const char* const name, int numOfBaseClasses, int* numOfBaseMethods);
	virtual ~BaseLayer() override;
protected:
	// レイヤードオブジェクトの初期化
	virtual void* InitializeLayerdObject(void* obj, int classID) override;
};

} // namespace Generated {}
} // namespace RTCOP {}

#endif //!__RTCOP_GENERATED_JAPANESELAYER__
