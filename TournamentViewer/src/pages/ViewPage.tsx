import { Tab } from "@headlessui/react";
import { classNames } from "./ManagePage";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faCircle } from "@fortawesome/free-solid-svg-icons";

export default function ViewPage() {
  return (
    <div>
      <h1 className="text-3xl text-center">Tournament settings</h1>
      <Tab.Group>
        <Tab.List className="flex flex-row gap-10 border-b mt-5">
          <Tab
            className={({ selected }) =>
              classNames(
                "py-2 px-4 text-lg",
                selected
                  ? "border-b-2 border-blue-500 font-bold text-blue-500"
                  : "text-gray-500"
              )
            }
          >
            <div className="flex flex-row gap-3 items-center">
              <FontAwesomeIcon
                icon={faCircle}
                className="text-red-500 text-sm animate-pulse"
              />
              <span>LIVE</span>
            </div>
          </Tab>
          <Tab
            className={({ selected }) =>
              classNames(
                "py-2 px-4 text-lg",
                selected
                  ? "border-b-2 border-blue-500 font-bold text-blue-500"
                  : "text-gray-500"
              )
            }
          >
            History
          </Tab>
        </Tab.List>
        <Tab.Panels className="mt-3">
          <Tab.Panel>LIVE</Tab.Panel>
          <Tab.Panel>HISTORY</Tab.Panel>
        </Tab.Panels>
      </Tab.Group>
    </div>
  );
}
