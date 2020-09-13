//================================================================================
// EnglishLayer.h
//
// 役割: EnglishLayerを表すクラス。自動生成。
//================================================================================

#ifndef __RTCOP_GENERATED_ENGLISHLAYER__
#define __RTCOP_GENERATED_ENGLISHLAYER__

// 機能実現のためのヘッダ
#include "RTCOP/Core/Layer.h"
#include "RTCOP/Core/LayerdObject.h"
#include "RTCOP/Core/PartialClassMembers.h"
// EnglishLayerに含まれるクラスのヘッダ
#include "Hello.h"

namespace RTCOP {
namespace Generated {

// Englishレイヤ
class EnglishLayer : public RTCOP::Core::Layer
{
public:
	// インスタンスの取得
	static EnglishLayer* GetInstance();
public:
	// コンストラクタ・デストラクタ
	explicit EnglishLayer(const int id, const char* const name, int numOfBaseClasses, int* numOfBaseMethods);
	virtual ~EnglishLayer() override;
protected:
	// レイヤードオブジェクトの初期化
	virtual void* InitializeLayerdObject(void* obj, int classID) override;
protected:
	// イベント発生時に実行されるメソッド
	virtual void _RTCOP_OnActivating() override;		// アクティベート開始時に実行される
	virtual void _RTCOP_OnActivated() override;			// アクティベート終了時に実行される
	virtual void _RTCOP_OnDeactivating() override;		// ディアクティベート開始時に実行される
	virtual void _RTCOP_OnDeactivated() override;		// ディアクティベート終了時に実行される
};

// EnglishレイヤのHello
class EnglishLayer_Hello : public RTCOP::Core::LayerdObject<baselayer::Hello>
{
public:
	// パーシャルクラスのメンバ変数
	class PartialClassMembers : public RTCOP::Core::PartialClassMembers
	{
	public:
		int _EnglishMember;
	};
public:
	EnglishLayer_Hello(int a);
	virtual void Print() override;
private:
	void _RTCOP_InitializePartialClass();
	void _RTCOP_FinalizePartialClass();
public:
	friend EnglishLayer;
};

} // namespace Generated {}
} // namespace RTCOP {}

#endif //!__RTCOP_GENERATED_ENGLISHLAYER__
