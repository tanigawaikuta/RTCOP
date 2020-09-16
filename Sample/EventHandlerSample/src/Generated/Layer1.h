#ifndef __RTCOP_GENERATED_LAYER1__
#define __RTCOP_GENERATED_LAYER1__

#include "RTCOP/Core/Layer.h"
#include "RTCOP/Core/LayerdObject.h"
#include "RTCOP/Core/PartialClassMembers.h"


#include "./Generated/BaseLayer.h"

namespace RTCOP {
namespace Generated {

class Layer1 : public RTCOP::Core::Layer
{
public:
	static Layer1* GetInstance();
	explicit Layer1(const int id, const char* const name, int numOfBaseClasses, int* numOfBaseMethods);
	virtual ~Layer1() override;
protected:
	virtual void* InitializeLayerdObject(void* obj, int classID) override;
	virtual void _RTCOP_OnActivating() override;
	virtual void _RTCOP_OnActivated() override;
	virtual void _RTCOP_OnDeactivating() override;
	virtual void _RTCOP_OnDeactivated() override;
};

} // namespace Generated {}
} // namespace RTCOP {}

namespace Layer1
{
class Sample : public RTCOP::Core::LayerdObject<baselayer::Sample>
{
public:
	friend RTCOP::Generated::Layer1;
	class PartialClassMembers : public RTCOP::Core::PartialClassMembers
	{
	public:
	};
	Sample ();
private:
	void _RTCOP_InitializePartialClass();
	void _RTCOP_FinalizePartialClass();
	private:
	void Initialize ();
	void Finalize ();

};
void OnActivating ();
void OnActivated ();
void OnDeactivating ();
void OnDeactivated ();

}


#endif
